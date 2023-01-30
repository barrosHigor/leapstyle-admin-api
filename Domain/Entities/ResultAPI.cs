using System;

namespace Domain.Entities
{
    public class ResultAPI
    {
        public int total { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
        public Object data { get; set; }
        public Object errors { get; set; }
    }
}
