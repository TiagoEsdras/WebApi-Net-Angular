﻿namespace ProEventos.Persistence.Models
{
    public class PageParams
    {
        public const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public string Term { get; set; }
    }
}