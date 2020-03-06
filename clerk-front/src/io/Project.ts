import axios from 'axios'
import {fromRaw, NewProject, Project, RawProject} from "@/model/Project";
import {Experiment, fromRawExperiment, RawExperiment} from "@/model/Experiment";

export async function getAllProject(): Promise<Project[]> {
    const projects = await axios.get<RawProject[]>('/api/project').then(res => res.data);
    console.log(projects[0]);
    return projects.map(fromRaw);
}

export async function addNewProject(n: NewProject): Promise<Project> {
    const project = await axios.post<RawProject>('/api/project', n).then(res => res.data);
    return fromRaw(project);
}

export async function getProjectById(id: number): Promise<Project> {
    const project = await axios.get<RawProject>(`/api/project/${id}`).then(res => res.data);
    return fromRaw(project);
}

export async function updateProjectComment(id: number, comment: string): Promise<Project> {
    const newComment = {
        "comment": comment
    };
    const project = await axios.patch<RawProject>(`/api/project/${id}/comment`, newComment).then(res => res.data);
    return fromRaw(project);
}

export async function getRunning(id: number): Promise<Experiment[]> {
    const raws = await axios.get<RawExperiment[]>(`/api/project/${id}/running`).then(res => res.data);
    return raws.map(fromRawExperiment);
}

export async function getAllExperiment(projectId: number): Promise<Experiment[]> {
    const raws = await axios.get<RawExperiment[]>(`/api/project/${projectId}/experiment`).then(res => res.data);
    return raws.map(fromRawExperiment);
}


