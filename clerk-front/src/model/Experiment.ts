import {parseUTCTimeStamp} from "@/utils";

export interface RawExperiment {
    id: number;
    projectId: number;
    name: string;
    configName: string;
    finished: boolean;
    total: number | null;
    progress: number;
    driver: number;
    command: string;

    startTime: string;
    lastProgress: string;

    gitHash: number;
    comment: number;

}

export interface Experiment {
    id: number;
    projectId: number;
    name: string;
    configName: string;
    finished: boolean;
    total: number | null;
    progress: number;
    driver: number;
    command: string;

    startTime: Date;
    lastProgress: Date;

    gitHash: number;
    comment: number;
}

export interface Progress {
    total: number;
    progress: number;
}

export function fromRawExperiment(raw: RawExperiment): Experiment {
    return {
        ...raw,
        startTime: parseUTCTimeStamp(raw.startTime),
        lastProgress: parseUTCTimeStamp(raw.startTime),
    }
}
