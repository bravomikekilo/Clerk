import {Experiment, Progress} from "@/model/Experiment";
import {getConn} from "@/signalr/signalr";


export async function watchExperiment(id: number) {
    const conn = await getConn();
    await conn.invoke('watchExperiment', id);
}

export async function ignoreExperiment(id: number, callback: (progress: Progress) => void) {
    const conn = await getConn();
    await conn.invoke('IgnoreExperiment', id);
    conn.off('UpdateProgress', callback);
}
