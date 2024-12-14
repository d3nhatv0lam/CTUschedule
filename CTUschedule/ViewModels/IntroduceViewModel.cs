using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTUschedule.Models;

namespace CTUschedule.ViewModels
{
    public partial class IntroduceViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; }
        //public ObservableCollection<Node> SelectedNodes { get; }

        private List<ViewModelBase> PageViewModels;
        [ObservableProperty]
        private ViewModelBase _currentViewModel;


        public Node _selectedNodes;
        public Node SelectedNodes
        {
            get => _selectedNodes;
            set
            {
                if (_selectedNodes == value) return;
                _selectedNodes = value;
                //ChangeView(SelectedNodes);
            }
        }

        public IntroduceViewModel()
        {
            PageViewModels = new List<ViewModelBase>()
            {

            };
            //SelectedNodes = new ObservableCollection<Node>();
            Nodes = new ObservableCollection<Node>()
            {
                new Node(0,"NotebookEditOutline","Giới thiệu"),
                new Node(1,"BookPlusOutline","Danh Mục HP"),
                new Node(2,"Calendar","Thời khóa biểu")
            };
        }

        //public void ChangeView(Node node)
        //{
        //    MainWindowViewModel.Instance.ChangeViewFromIndex(node.Id);
        //}
    }
}
