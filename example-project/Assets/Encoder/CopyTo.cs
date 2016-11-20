using System.IO;

			public static partial class Extensions
			{
				public static void CopyTo(this Stream source, Stream target)
				{
					byte[] buffer = new byte[8192];
					int rc;
					while ((rc = source.Read(buffer, 0, buffer.Length)) > 0)
						target.Write(buffer, 0, rc);
				}
			}
		





