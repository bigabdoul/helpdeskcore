using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HelpDeskCore.Models
{
  public class FormFileReader
  {
    IFormFile _file;

    public FormFileReader(IFormFile file)
    {
      _file = file ?? throw new System.ArgumentNullException(nameof(file));
    }

    public async Task<string> ReadAsStringAsync(Encoding enc = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      var buffer = await ReadAsByteArrayAsync(cancellationToken);
      return (enc ?? GetEncoding()).GetString(buffer, 0, buffer.Length);
    }

    public async Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      using (var s = _file.OpenReadStream())
      {
        var read = 0; // the number of bytes read in each loop
        var offset = 0; // the position at which to start writing content within the buffer
        var buffer = new byte[_file.Length]; // receives the file's content
        const int BUF_SZ = 1024; // the maximum number of bytes to read in each loop
        do
        {
          // read the stream's content in chunks and write each one to the given buffer;
          // the offset is incremented gradually by the number of bytes read in each loop
          // therefore defines the next write position within the buffer
          offset += (read = await s.ReadAsync(buffer, offset, BUF_SZ, cancellationToken));
        } while (read > 0);
        return buffer;
      }
    }

    Encoding GetEncoding()
    {
      Encoding enc = null;
      var fenc = _file.Headers["Content-Encoding"];
      if (fenc.Count > 0  && !string.IsNullOrWhiteSpace(fenc[0]))
      {
        enc = Encoding.GetEncoding(fenc[0]);
      }
      return enc ?? Encoding.UTF8;
    }
  }
}
