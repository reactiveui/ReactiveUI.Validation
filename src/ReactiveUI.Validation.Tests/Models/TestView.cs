namespace ReactiveUI.Validation.Tests.Models
{
    public class TestView : IViewFor<TestViewModel>
    {
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TestViewModel;
        }

        public TestViewModel ViewModel { get; set; }

        public string NameLabel { get; set; }
        
        public string NameLabelError { get; set; }

        private TestView()
        {
        }

        public TestView(TestViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}