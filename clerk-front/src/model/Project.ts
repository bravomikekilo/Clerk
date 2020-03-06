
export interface Project {
    id: number;
    name: string;
    createTime: Date;
    comment: string | null;
}

export interface RawProject {
    id: number;
    name: string;
    createTime: string;
    comment: string | null;
}

export interface NewProject {
    name: string;
    comment: string | null;
}

export function fromRaw(raw: RawProject): Project {
    const date = new Date(raw.createTime + 'Z');
    return {
        ...raw,
        createTime: date,
    }
}
