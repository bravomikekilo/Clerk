import {getConn} from "@/signalr/signalr";


export async function watchLog(id: number): Promise<boolean> {
    const conn = await getConn();
    return await conn.invoke("ReadLog", id)
}

export async function ignoreLog(id: number) {
    const conn = await getConn();
    await conn.invoke("IgnoreLog", id)
}
