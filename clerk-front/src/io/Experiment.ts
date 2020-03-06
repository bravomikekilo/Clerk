import {Experiment, fromRawExperiment, Progress, RawExperiment} from "@/model/Experiment";
import axios from 'axios'
import {LogMessage} from "@/model/LogMessage";

export async function finishExperiment(id: number): Promise<void> {
    await axios.patch(`/api/experiment/${id}/finish`);
    return
}

export async function updateExperimentComment(id: number, comment: string): Promise<Experiment> {
    const raw = await axios.patch<RawExperiment>(`/api/experiment/${id}/comment`, {
        comment
    }).then(res => res.data);
    return fromRawExperiment(raw);
}

export async function updateExperimentProgress(id: number, total: number, progress: number): Promise<void> {
    const res = await axios.patch<Progress>(`api/experiment/${id}/progress`, {
        total,
        progress
    }).then(r => r.data);
    return;
}

export async function getExperimentById(id: number): Promise<Experiment> {
    const raw = await axios.get<RawExperiment>(`/api/experiment/${id}`)
        .then(res => res.data);
    return fromRawExperiment(raw);
}

export async function getExperimentLog(id: number): Promise<LogMessage> {
    return await axios.get<LogMessage>(`/api/experiment/${id}/log`)
        .then(res => res.data);
}
