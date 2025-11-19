using CommunityToolkit.Mvvm.ComponentModel;

namespace Hourregistration.Core.Models
{
    public abstract partial class Model(int id, string name) : ObservableObject
    {
        public int Id { get; set; } = id;
        [ObservableProperty]
        public string name = name;
    }
}