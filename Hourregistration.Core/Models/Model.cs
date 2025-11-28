using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection.Metadata.Ecma335;

namespace Hourregistration.Core.Models
{
    public abstract partial class Model(int id) : ObservableObject
    {
        public int Id { get; set; } = id;
    }
}