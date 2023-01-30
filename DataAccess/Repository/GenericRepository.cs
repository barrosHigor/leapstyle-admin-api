using Dapper;
using Dapper.Contrib.Extensions;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;

namespace DataAccess.Repository
{
    public class ParametrosDapper
    {
        public string key { get; set; }
        public object value { get; set; }
    }

    public class GenericRepository<T> : IGenericServices<T> where T : class
    {
        protected static IDbConnection Conexao = null;

        public GenericRepository()
        {   
            Conexao = GetConexao();
        }

        protected IDbConnection GetConexao()
        {   
            return ProviderSystem.GetConexaoSqlServer();
        }

        protected static string getTableName()
        {
            var attrType = typeof(T);
            var tableAttribute = attrType.GetCustomAttributes().FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.TableAttribute;
            var tableAttributeDapper = attrType.GetCustomAttributes().FirstOrDefault() as TableAttribute;

            if (tableAttribute == null && tableAttributeDapper == null)
                throw new Exception($"Entity {attrType.Name} sem table name!");

            return tableAttribute != null ? tableAttribute.Name : tableAttributeDapper.Name;
        }

        private static bool verificaExistenciaPropriedade(string nomePropriedade)
        {
            var properties = typeof(T).GetProperties();
            var temPropriedade = false;
            foreach (var property in properties)
            {
                if (property.Name == nomePropriedade)
                    temPropriedade = true;
            }
            return temPropriedade;
        }

        private string getMostrarExcluidos(string alias) => $@" AND {alias}.deleted_at IS NULL ";

        private string tratarQuery(string query)
        {
            query = query.Replace("SELECT", "").Replace("Select", "").Replace("select", "");
            query = query.Replace("DROP", "").Replace("Drop", "").Replace("drop", "");
            query = query.Replace("CREATE", "").Replace("Create", "").Replace("create", "");
            query = query.Replace("TABLE", "").Replace("Table", "").Replace("table", "");
            query = query.Replace("COLUMN", "").Replace("Column", "").Replace("column", "");
            query = query.Replace("UPDATE", "").Replace("Update", "").Replace("update", "");
            query = query.Replace("INSERT", "").Replace("Insert", "").Replace("insert", "");
            query = query.Replace("-- ", "").Replace(";", "").Replace("'", "").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

            return query;
        }

        protected (string where, string tableName, string orderBy, DynamicParameters whereParameters) getDadosToQuery(FiltroAPI filtros)
        {
            var tableName = getTableName();
            var existePropriedadeAtivo = verificaExistenciaPropriedade("ativo");
            var where = $"";
            var whereDictionary = new List<ParametrosDapper>();
            var whereParameters = new DynamicParameters();

            if (existePropriedadeAtivo)
                where += $" AND {tableName}.ativo = true ";

            var orderBy = $"{tableName}.id";
            var mostrarExcluido = filtros != null && filtros.flagMostrarExcluido ? "" : getMostrarExcluidos(tableName);

            where += mostrarExcluido;

            if (filtros != null)
            {
                where = "";
                var tipoFiltro = "";
                var filtroSrt = "";
                var alias = "";
                var aliasTabelaPrincipal = filtros.aliasTabelaPrincipal != null ? filtros.aliasTabelaPrincipal : tableName;
                var nomeCampoData = "data_inicio";

                if (filtros.filtros != null)
                    foreach (var filtro in filtros.filtros)
                    {
                        alias = tableName;
                        alias = filtro.alias?.Length > 0 ? $"{filtro.alias}." : filtro.nomeCampo.Contains(".") ? "" : $"{alias}.";
                        tipoFiltro = Filtro.getCondicaoFiltro(filtro.condicao);
                        filtroSrt = tipoFiltro == "like" ? $"%{tratarQuery(filtro.valorCampo)}%" : tratarQuery(filtro.valorCampo);

                        if (filtro?.tipoFiltro == Filtro.TipoFiltro.DATE)
                        {
                            if (tipoFiltro == "<=" || tipoFiltro == "<")
                                nomeCampoData = "data_fim";

                            where += $"{Filtro.getTipoFiltro(filtro.tipo)} Date({alias}{filtro.nomeCampo}) {tipoFiltro} @{nomeCampoData} ";
                            whereDictionary.Add(new ParametrosDapper() { key = nomeCampoData, value = filtroSrt });
                        }
                        else if (filtro?.tipoFiltro == Filtro.TipoFiltro.BOOLEAN)
                        {
                            where += $"{Filtro.getTipoFiltro(filtro.tipo)} {alias}{filtro.nomeCampo} {tipoFiltro} @{filtro.nomeCampo} ";
                            whereDictionary.Add(new ParametrosDapper() { key = filtro.nomeCampo, value = Boolean.Parse(filtroSrt) });
                        }
                        else if (filtro?.tipoFiltro == Filtro.TipoFiltro.NUMBER)
                        {
                            where += $"{Filtro.getTipoFiltro(filtro.tipo)} {alias}{filtro.nomeCampo} {tipoFiltro} @{filtro.nomeCampo} ";
                            whereDictionary.Add(new ParametrosDapper() { key = filtro.nomeCampo, value = filtroSrt });
                        }
                        else
                        {
                            where += $"{Filtro.getTipoFiltro(filtro.tipo)} {alias}{filtro.nomeCampo} {tipoFiltro} @{filtro.nomeCampo} ";
                            whereDictionary.Add(new ParametrosDapper() { key = filtro.nomeCampo, value = filtroSrt });
                        }
                    }

                foreach (var item in whereDictionary)
                    whereParameters.Add(item.key, item.value);

                if (existePropriedadeAtivo && filtros.flagSomenteAtivo)
                    where += $" AND {aliasTabelaPrincipal}.ativo = true";

                where += filtros.flagMostrarExcluido ? "" : getMostrarExcluidos(aliasTabelaPrincipal);

                if (filtros.ordenacao != null)
                    orderBy = filtros.ordenacao;
                else
                    orderBy = $"{aliasTabelaPrincipal}.id";
            }

            return (where, tableName, orderBy, whereParameters);
        }

        private T ToObject<T>(IDictionary<string, object> source)
        where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public virtual bool Delete(T obj)
        {
            using (var conexao = GetConexao())
            {
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();

                var transaction = conexao.BeginTransaction();
                try
                {
                    var result = conexao.Update(obj);
                    transaction.Commit();

                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.Utils.salvaLog(ex, "Delete");
                    throw;
                }
                finally
                {
                    conexao.Close();
                    conexao.Dispose();
                }
            }
        }

        public virtual void Delete(string cmd)
        {
            using (var conexao = GetConexao())
            {
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();

                var transaction = conexao.BeginTransaction();
                try
                {
                    conexao.Query(cmd);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.Utils.salvaLog(ex, "Delete");
                    throw;
                }
                finally
                {
                    conexao.Close();
                    conexao.Dispose();
                }
            }
        }

        public virtual long Insert(T obj)
        {
            using (var conexao = GetConexao())
            {
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();

                var transaction = conexao.BeginTransaction();
                try
                {
                    var id = conexao.Insert(obj);
                    transaction.Commit();

                    return id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.Utils.salvaLog(ex, "Insert T");
                    throw;
                }
                finally
                {
                    conexao.Close();
                    conexao.Dispose();
                }
            }
        }

        public virtual long Insert(List<T> objs)
        {
            using (var conexao = GetConexao())
            {
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();

                var transaction = conexao.BeginTransaction();
                try
                {
                    var result = conexao.Insert(objs);
                    transaction.Commit();

                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.Utils.salvaLog(ex, "Insert List<T>");
                    throw;
                }
                finally
                {
                    conexao.Close();
                    conexao.Dispose();
                }
            }
        }

        public virtual long InsertOrUpdate(T obj)
        {
            using (var conexao = GetConexao())
            {
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();

                var transaction = conexao.BeginTransaction();
                try
                {
                    if (conexao.Update(obj))
                    {
                        transaction.Commit();
                        return 0;
                    }
                    
                    var result = conexao.Insert(obj);
                    transaction.Commit();

                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.Utils.salvaLog(ex, "InsertOrUpdate");
                    throw;
                }
                finally
                {
                    conexao.Close();
                    conexao.Dispose();
                }
            }
        }

        public virtual ResultAPI Select(int skip, int take, FiltroAPI filtros)
        {
            try
            {
                var dadosQuery = getDadosToQuery(filtros);
                var queryDados = $@"SELECT * FROM {getTableName()} WHERE id > 0 {dadosQuery.where} ORDER BY {dadosQuery.orderBy} {(skip != -1 ? $" LIMIT {take} OFFSET {skip}" : "")}";
                var queryTotal = $@"SELECT COUNT(*) FROM {getTableName()} WHERE id > 0 {dadosQuery.where} ORDER BY {dadosQuery.orderBy}";
                var dados = new List<T>();
                var total = new List<int>();

                using (var conexao = GetConexao())
                {
                    dados = conexao.Query<T>(queryDados, dadosQuery.whereParameters).ToList();
                    total = conexao.Query<int>(queryTotal, dadosQuery.whereParameters).ToList();
                }

                var result = new ResultAPI()
                {
                    total = total.FirstOrDefault(),
                    skip = skip,
                    take = take,
                    data = dados
                };

                return result;
            }
            catch (Exception ex)
            {
                Utils.Utils.salvaLog(ex, "Select");
                throw;
            }
        }

        public virtual async Task<ResultAPI> SelectAsync(int skip, int take, FiltroAPI filtros)
        {
            try
            {
                var dadosQuery = getDadosToQuery(filtros);
                var tableName = filtros?.aliasTabelaPrincipal != null && filtros?.aliasTabelaPrincipal != "" ? $"{getTableName()} {filtros.aliasTabelaPrincipal}" : getTableName();
                var queryDados = $@"SELECT * FROM {tableName} WHERE id > 0 {dadosQuery.where} ORDER BY {dadosQuery.orderBy} {(skip != -1 ? $" LIMIT {take} OFFSET {skip}" : "")}";
                var queryTotal = $@"SELECT COUNT(*) FROM {tableName} WHERE id > 0 {dadosQuery.where} ORDER BY {dadosQuery.orderBy}";
                var dados = new List<T>();
                var total = new List<int>();

                using (var conexao = GetConexao())
                {
                    dados = (await conexao.QueryAsync<T>(queryDados, dadosQuery.whereParameters)).ToList();
                    total = (await conexao.QueryAsync<int>(queryTotal, dadosQuery.whereParameters)).ToList();
                }

                var result = new ResultAPI()
                {
                    total = total.FirstOrDefault(),
                    skip = skip,
                    take = take,
                    data = dados
                };

                return result;
            }
            catch (Exception ex)
            {
                Utils.Utils.salvaLog(ex, "SelectAsync");
                throw;
            }
        }

        public virtual T SelectById(object id)
        {
            try
            {
                using (var conexao = GetConexao())
                {
                    return conexao.Get<T>(id);
                }
            }
            catch (Exception ex)
            {
                Utils.Utils.salvaLog(ex, "SelectById");
                throw;
            }
        }

        public virtual ResultAPI Select(string cmd)
        {
            try
            {
                using (var conexao = GetConexao())
                {
                    var dados = conexao.Query<T>(cmd);
                    var result = new ResultAPI() { data = dados };

                    return result;
                }
            }
            catch (Exception ex)
            {
                Utils.Utils.salvaLog(ex, "Select", cmd);
                throw;
            }
        }

        public virtual bool Update(T obj)
        {
            using (var conexao = GetConexao())
            {

                if (conexao.State != ConnectionState.Open)
                    conexao.Open();

                var transaction = conexao.BeginTransaction();
                try
                {
                    var result = conexao.Update(obj);
                    transaction.Commit();

                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.Utils.salvaLog(ex, "Update");
                    throw;
                }
                finally
                {
                    conexao.Close();
                    conexao.Dispose();
                }
            }
        }
    }
}
