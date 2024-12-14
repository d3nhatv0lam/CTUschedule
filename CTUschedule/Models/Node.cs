using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.Models
{
    public class Node
    {
        public ObservableCollection<Node>? SubNodes { get; }
        public string ImageKind { get; }
        public string Title { get; }
        public int Id { get; }

        public Node(int id,string imageKind,string title)
        {
            Id = id;
            ImageKind = imageKind;
            Title = title;
        }

        public Node(int id, string imageKind, string title, ObservableCollection<Node> subNodes)
        {
            Id = id;
            ImageKind = imageKind;
            Title = title;
            SubNodes = subNodes;
        }
    }
}
