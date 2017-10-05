using System;
using System.IO;
using System.Linq;
using Grasshopper.Kernel;
using GH_IO.Serialization;

namespace GrasshopperGit
{
	public class GrasshopperFolder : GH_Component
	{
		public GrasshopperFolder() : base("GrasshopperFolder", "Folder", "Write Active Script to Folder", "File", "GHIO") { }
		public override Guid ComponentGuid => new System.Guid("{533CCFAE-A84C-4A7A-8C12-5074231D28AE}");
		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			pManager.AddBooleanParameter("Serialize", "S", "Serialize", GH_ParamAccess.item);
		}
		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{

		}
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			bool enabled = false;
			if (DA.GetData<bool>(0, ref enabled))
			{
				GH_Document document = this.OnPingDocument();
				if (document == null)
				{
					base.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid Document");
					return;
				}
				string filePath = document.FilePath;
				if (string.IsNullOrEmpty(filePath))
				{
					base.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Document not saved yet!");
					return;

				}
				string directory = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
				DirectoryInfo directoryInfo = Directory.CreateDirectory(directory);
				if (directoryInfo == null)
					directoryInfo = new DirectoryInfo(directory);
				foreach (FileInfo file in directoryInfo.GetFiles())
					file.Delete(); // Should ignore git files

				foreach (GH_Component component in Grasshopper.Instances.ActiveCanvas.Document.Objects.OfType<GH_Component>())
				{
					GH_Archive archieve = new GH_Archive();
					archieve.AppendObject(component, "Component");
					archieve.WriteToFile(Path.Combine(directory, component.InstanceGuid.ToString("N") + ".ghx"), true, false);
				}
			}

		}
	}
}
