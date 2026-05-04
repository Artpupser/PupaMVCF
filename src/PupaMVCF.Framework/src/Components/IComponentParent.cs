using System.Text;

namespace PupaMVCF.Framework.Components;

public interface IComponentParent {
   public StringBuilder Builder { get; }
   public View CurrentView { get; }
}