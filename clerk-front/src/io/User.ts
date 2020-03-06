import axios from 'axios'

export async function signUp(username: string, password: string, email: string): Promise<void> {

    await axios.post(`/api/user/sign-up`, {
        username: username,
        password: password,
        email: email
    }, {withCredentials: true});

    return;
}

export async function signIn(username: string, password: string): Promise<void> {

    /*
    await fetch('/api/user/sign-in', {
        method: 'POST',
        body: JSON.stringify({username, password}),
        credentials: 'include',
        headers: new Headers({
            'Content-Type': 'application/json'
        })
    }).then(res => res.json());
     */

    await axios.post(`/api/user/sign-in`, {
        username: username,
        password: password
    }, {withCredentials: true})
        .then(res => {
            console.log(res.headers['set-cookie'])
        });
    return;
}


export async function refresh(): Promise<void> {
    await axios.get('api/user/refresh').then(res => res.data);
    return;
}
