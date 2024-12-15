using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTUschedule.Models;
using System.Diagnostics;
using System.Security.Cryptography;

namespace CTUschedule.ViewModels
{
    public partial class MainHomeViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; }

        private List<ViewModelBase> PageViewModels;
        [ObservableProperty]
        private ViewModelBase _currentViewModel;


        public Node _selectedNode;
        public Node SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (_selectedNode == value) return;
                _selectedNode = value;
                ChangeView(SelectedNode);
            }
        }

        public MainHomeViewModel()
        {
            PageViewModels = new List<ViewModelBase>()
            {
                new IntroduceViewModel(),
                new CourseListViewModel(),
            };

            CurrentViewModel = PageViewModels.First();
            Nodes = new ObservableCollection<Node>()
            {
                new Node(0,"NotebookEditOutline","Giới thiệu"),
                new Node(1,"BookPlusOutline","Danh mục HP"),
                new Node(2,"Calendar","Thời khóa biểu")
            };
            //SelectedNode = new ObservableCollection<Node>() { Nodes.First() };
            SelectedNode = Nodes.First();
        }

        private void ChangeView(Node node)
        {
            int index = node.Id;
            if (index < 0 || index >= PageViewModels.Count) return;
            CurrentViewModel = PageViewModels[index];

        }
    }
}
