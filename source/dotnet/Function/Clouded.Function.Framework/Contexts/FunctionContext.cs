using System.Net;
using Clouded.Function.Framework.Contexts.Base;

namespace Clouded.Function.Framework.Contexts;

public class FunctionContext : HookContext
{
    public HttpWebRequest Request { get; set; }
}
