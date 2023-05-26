using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammInterface
{
    public class Vertex
    {
        public int Id;
        public List<Vertex> BindedVertices { get; set; } = new();
        public Vertex(int id)
        {
            Id = id;
        }
    }
}
