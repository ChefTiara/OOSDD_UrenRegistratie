using CommunityToolkit.Mvvm.ComponentModel;

namespace Hourregistration.Core.Models
{
    public abstract partial class Model(int id) : ObservableObject
    {
        public int Id { get; set; } = id;
    }
}