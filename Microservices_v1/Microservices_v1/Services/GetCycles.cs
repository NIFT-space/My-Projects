using Dapper;
using Microservices_v1.Connection;

namespace Microservices_v1.Services
{
    public class GetCycles
    {
        public List<Cycles> FetchCyc()
        {
            List<Cycles> cyc = new List<Cycles>();

            Conn cc2 = new Conn();
            var res = cc2.GetConn<Cycles>("SELECT cycle_no as cycleid,cycle_desc as cycname FROM cycles");

            foreach(var c in res)
            {
                cyc.Add(c);
            }

            return cyc;
        }
        public class Cycles
        {
            public int cycleid { get; set; }
            public string cycname { get; set; }

        }
    }
}
