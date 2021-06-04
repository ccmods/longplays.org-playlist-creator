using System.Collections.Generic;

public class AaData
    {
        public string date { get; set; }
        public string video { get; set; }
        public string game { get; set; }
        public string system { get; set; }
        public string length { get; set; }
        public string region { get; set; }
        public string language { get; set; }
        public string subtitle { get; set; }
        public string info { get; set; }
        public string players { get; set; }
    }

    public class Videos
    {
        public int draw { get; set; }
        public string iTotalRecords { get; set; }
        public string iTotalDisplayRecords { get; set; }
        public List<AaData> aaData { get; set; }
    }
