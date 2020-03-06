<template>
  <v-content class="fill-height">
    <v-row class="justify-center" v-if="project === null">
      <v-col cols="12" sm="8" md="8">
        <v-row class="mx-0" align="center">
          <h1 class="display-1 my-5">#{{projectId}} </h1>
          <v-divider vertical class="mx-3"/>
          <h1> Loading</h1>
          <v-progress-circular class="mx-5" indeterminate color="primary"/>
        </v-row>
      </v-col>
    </v-row>
    <v-row v-else justify="center" class="mx-0">
      <v-col cols="12" sm="8" md="8">
        <v-row class="mx-0" justify="space-between" align="center">
          <h1 class="display-1 my-5">#{{project.id}} {{project.name}}</h1>
          <h3 class="subtitle-1 my-5">
            Created {{project.createTime.toLocaleDateString()}} {{project.createTime.toLocaleTimeString()}}
          </h3>
        </v-row>
        <v-divider></v-divider>
        <comment-editor @edit="editComment" :comment="project.comment" class="my-5"></comment-editor>

        <template v-if="Object.keys(this.runningExperiment).length !== 0">
          <v-divider></v-divider>
          <h1 class="headline my-5">Running experiment</h1>
          <running-experiment class="my-5" v-for="(running) in runningExperiment"
                              :key="running.id"
                              :experiment="running"/>
        </template>
        <v-divider class="my-5"></v-divider>
        <h1 class="headline my-5">All Experiments</h1>
        <v-card>
          <v-card-title>
            <v-spacer/>
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              label="Search"
              single-line
              hide-details
            ></v-text-field>
          </v-card-title>
          <v-data-table
            :headers="headers"
            :search="search"
            :items="allExperiments"
          ></v-data-table>
        </v-card>
      </v-col>
    </v-row>
  </v-content>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from 'vue-property-decorator'
    import CommentEditor from "@/components/CommentEditor.vue";
    import RunningExperiment from "@/components/RunningExperiment.vue";
    import {Project} from "@/model/Project";
    import {getAllExperiment, getProjectById, getRunning, updateProjectComment} from "@/io/Project";
    import {Experiment, Progress} from "@/model/Experiment";
    import {connection, getConn} from "@/signalr/signalr";
    import {watchExperiment} from "@/signalr/experiment";

    @Component({
        components: {
            CommentEditor,
            RunningExperiment,
        }
    })
    export default class ProjectView extends Vue {
        @Prop() projectId!: number;
        project: Project | null = null;
        runningExperiment: {[key: number]: Experiment} = {};
        allExperiments: Experiment[] = [];

        search = '';
        headers = [
            {
                text: 'id',
                align: 'start',
                sortable: true,
                value: 'id'
            },
            {
                text: 'name',
                align: 'start',
                sortable: false,
                value: 'name'
            },
            {
                text: 'driver',
                align: 'start',
                sortable: false,
                value: 'driver'
            },
            {
                text: 'config name',
                align: 'start',
                sortable: false,
                value: 'configName'
            },
            {
                text: 'start time',
                align: 'start',
                sortable: false,
                value: 'startTime'
            }
        ];

        async created() {
            this.project = await getProjectById(this.projectId);

            const runningExperimentList = await getRunning(this.projectId);
            const running: {[key: number]: Experiment} = {};
            runningExperimentList.forEach((v) => {
                running[v.id] = v;
            });
            this.runningExperiment = running;

            const conn = await getConn();
            conn.on('UpdateProgress', this.updateProgress);
            for (const exper of runningExperimentList) {
                await watchExperiment(exper.id);
            }

            this.allExperiments = await getAllExperiment(this.projectId);
        }

        updateProgress(id: number, progress: Progress) {
            const exper = this.runningExperiment[id];
            if (exper !== undefined) {
                exper.total = progress.total;
                exper.progress = progress.progress;
            }
        }

        async editComment(newComment: string) {
            console.log('change comment');
            const project = await updateProjectComment(this.projectId, newComment);
            this.project = project
        }

        async beforeDestroy() {
            connection.off('UpdateProgress', this.updateProgress)
        }

    }
</script>

<style scoped>

</style>
