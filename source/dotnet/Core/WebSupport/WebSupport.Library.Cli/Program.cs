// See https://aka.ms/new-console-template for more information

using WebSupport.Library;
using WebSupport.Library.Dtos;

var client = new WebSupportClient(
    "186093f6-edeb-4531-aa9b-249492ca7e8f",
    "fb2423dd320493aabfdc4a3485c4c64ee91116a7"
);
var user = await client.User.Me();

if (user == null)
    return;

var domain = "clouded.sk";

await client.Dns.RecordCreate(
    domain,
    new RecordCreateInput
    {
        Type = "A",
        Name = "library-testovaanie",
        Content = "62.176.171.229",
        Ttl = 600
    }
);

Console.WriteLine("Hello, World!");
