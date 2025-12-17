using CommunityToolkit.Mvvm.ComponentModel;

namespace Hourregistration.Core.Models
{
    public abstract partial class Model(long id) : ObservableObject
    {
        public long Id { get; set; } = id;
    }
}