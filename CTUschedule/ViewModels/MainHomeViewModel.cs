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

        [ObservableProperty]
        private List<ViewModelBase> _pageViewModels;
        [ObservableProperty]
        private ViewModelBase _currentViewModel;
        [ObservableProperty]
        private bool _isChangingView = false;


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
                new CourseListEditViewModel(),
                new ScheduleViewModel(),
            };

            CurrentViewModel = PageViewModels.First();
            Nodes = new ObservableCollection<Node>()
            {
                new Node(0,"NotebookEditOutline","Giới thiệu"),
                new Node(1,"BookPlusOutline","Danh mục HP"),
                new Node(2,"NotebookEditOutline","Chỉnh sửa nhóm HP"),
                new Node(3,"Calendar","Thời khóa biểu"),
            };
            //SelectedNode = new ObservableCollection<Node>() { Nodes.First() };
            SelectedNode = Nodes.First();
        }

        private async void ChangeView(Node node)
        {
            IsChangingView = true;
            int index = node.Id;
            if (index < 0 || index >= PageViewModels.Count)
            {
                IsChangingView = false;
                return;
            }
            CurrentViewModel = PageViewModels[index];

            if (CurrentViewModel is CourseListViewModel viewModel)
            {
                //await Task.Run(() => viewModel.Init());
            }
            IsChangingView = false;
        }
    }
}
