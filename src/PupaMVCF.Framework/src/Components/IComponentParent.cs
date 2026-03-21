using System.Text;

using PupaMVCF.Framework.Views;

namespace PupaMVCF.Framework.Components;

public interface IComponentParent {
   public StringBuilder Builder { get; }
   public View CurrentView { get; }
}