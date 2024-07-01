import AuthClient from "@younics/clouded-auth-client";

const authClient = new AuthClient('http://localhost:8001', 'GXvxVmRpZouoT5sQMews7mpP9duBAkGGmrr2jsUJ8JbF3w7pDqmEbFwQ3f5qAa73ATdfmEKiJjDq6qdr2fmQ8p2gthDg4RujapA9atKTjhxQ888taYkkgZ3fP5KSB5nh')

async function getToken() {
    return await authClient.token({
        identity: "string",
        password: "string"
    }).catch(err => console.error(err));
}


async function run() {
    const authToken = await getToken();

    console.log({authToken})
}

run();
