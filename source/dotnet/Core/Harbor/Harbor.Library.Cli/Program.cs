// See https://aka.ms/new-console-template for more information

using Flurl.Http;
using Harbor.Library;
using Harbor.Library.Dtos;
using Harbor.Library.Enums;

var client = new HarborClient("<Server>", "<Username>", "<Password>");

try
{
    await client.User.Create(
        new UserInput
        {
            Name = "test",
            RealName = "asd asd",
            Email = "test@test.sk",
            Password = "testTest1$"
        }
    );

    await client.Project.Create(new ProjectInput { Name = "test", StorageLimit = -1 });

    await client.Project.AddMember(
        "test",
        new ProjectMemberAddInput
        {
            Member = new MemberInput { UserName = "test" },
            Role = ERole.Maintainer
        }
    );
}
catch (FlurlHttpException ex)
{
    var response = await ex.GetResponseStringAsync();
}

Console.WriteLine("Harbor CLI!");
