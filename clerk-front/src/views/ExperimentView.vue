<template>
  <v-content class="fill-height">
    <v-row dense class="fill-height mx-1 py-1">
      <v-col cols="12" md="4" class="fill-height">
        <v-card class="fill-height flex-col">
          <v-card-title>
            #{{this.experimentId}}
            <v-divider vertical class="mx-2"/>
            {{this.experiment === null ? '' : this.experiment.name}}
          </v-card-title>
          <v-divider/>
          <div v-if="experiment === null" class="text-center" style="padding-top: 30%">
            <v-progress-circular
              indeterminate
              size="200"
              width="10"
              color="primary"
            >
              Loading
            </v-progress-circular>
          </div>
          <v-card-text class="subtitle-1 fill-height flex-col" v-else>
            <span>
              <strong>Config name:</strong>
              {{this.experiment.configName}}
            </span>
            <v-divider class="my-2"/>
            <span>
              <strong>Driver:</strong>
              {{this.experiment.driver}}
            </span>
            <v-divider class="my-2"/>
            <strong>Command:</strong>
            {{this.experiment.command}}
            <v-divider class="my-2"/>
            <span>
              <strong>Start time:</strong>
              {{this.experiment.startTime.toLocaleString()}}
            </span>
            <v-divider class="my-2"/>
            <strong>Git hash:</strong>
            {{this.experiment.gitHash ? this.experiment.gitHash : "no git hash"}}
            <v-divider class="my-2"/>
            <code class="mt-5 fill-height" style="overflow: scroll">
              {{this.experiment.configContent}}
            </code>
          </v-card-text>
        </v-card>
      </v-col>

      <v-col cols="12" md="4" class="fill-height">
        <v-card class="fill-height flex-col">
          <v-card-title>Experiment Comment</v-card-title>
          <v-divider/>
          <div v-if="experiment === null" class="text-center" style="padding-top: 40%">
            <v-progress-circular
              indeterminate
              size="200"
              width="10"
              color="primary"
            >
              Loading
            </v-progress-circular>
          </div>
          <v-card-text v-else class="flex-rest flex-col">
            <comment-editor :comment="this.experiment.comment" class="flex-rest"/>
          </v-card-text>

        </v-card>

      </v-col>
      <v-col cols="12" md="4" class="fill-height flex-col">
        <v-card class="fill-height" v-if="experiment === null">
          <div class="text-center" style="padding-top: 40%">
            <v-progress-circular
              indeterminate
              size="200"
              width="10"
              color="primary"
            >
              Loading
            </v-progress-circular>
          </div>
        </v-card>
        <template v-else>
          <v-card class="mb-2">
            <v-card-title>
              Progress
              <v-spacer/>
              <v-chip color="amber" v-if="this.experiment.finished"
                      outlined>
                Finished
              </v-chip>
              <v-btn color="error" v-else>Finish</v-btn>
            </v-card-title>
            <v-card-text>
              <progress-bar
                :progress="experiment.progress"
                :total="experiment.total"
                :start-time="experiment.startTime"
                :finished="experiment.finished"
              />
            </v-card-text>

          </v-card>
          <v-card class="fill-height flex-col">
            <v-card-title>
              Log
              <v-spacer/>
              <v-btn color="indigo" dark>
                Download
                <v-icon right dark>mdi-download</v-icon>
              </v-btn>
            </v-card-title>
            <v-divider/>
            <textarea v-model="displayLog"
                      class="flex-rest ma-3 log-textarea"
                      readonly
                      style="resize: none;">
            </textarea>
          </v-card>
        </template>
      </v-col>
    </v-row>
  </v-content>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from "vue-property-decorator";
    import {Experiment, Progress} from "@/model/Experiment";
    import {getExperimentById, getExperimentLog, updateExperimentComment} from "@/io/Experiment";
    import CommentEditor from "@/components/CommentEditor.vue";
    import ProgressBar from "@/components/ProgressBar.vue";
    import {getConn, connection} from "@/signalr/signalr";
    import {watchExperiment} from "@/signalr/experiment";
    import {watchLog} from "@/signalr/log";

    @Component({
        components: {CommentEditor, ProgressBar}
    })
    export default class ExperimentView extends Vue {
        @Prop() experimentId!: number;
        experiment: Experiment | null = null;
        log = '';
        hasLog = true;

        get displayLog() {
            if (this.hasLog) {
                return this.log;
            } else {
                return "NO LOG";
            }
        }

        async updateExperiment() {
            this.experiment = await getExperimentById(this.experimentId)
        }

        async updateComment(comment: string) {
            await updateExperimentComment(this.experimentId, comment);
            await this.updateExperiment();
        }

        async created() {
            await this.updateExperiment();
            const conn = await getConn();
            conn.on('UpdateProgress', this.updateProgress);
            conn.on('AppendLog', this.updateLog);
            conn.on('ExperimentFinish', this.finishExperiment);
            await watchExperiment(this.experimentId);
            this.hasLog = await watchLog(this.experimentId);
        }

        updateProgress(id: number, progress: Progress) {
            if (id === this.experimentId) {
                if (this.experiment !== null) {
                    this.experiment.total = progress.total;
                    this.experiment.progress = progress.progress;
                }
            }
        }

        finishExperiment(id: number) {
            if (this.experimentId == id) {
                if (this.experiment !== null) {
                    this.experiment.finished = true;
                }
            }
        }

        updateLog(log: string) {
            this.hasLog = true;
            this.log += log
        }

        async beforeDestroy() {
            connection.off('UpdateProgress', this.updateProgress);
            connection.off('AppendLog', this.updateProgress)
        }


    }
</script>

<style scoped>

  .flex-col {
    display: flex;
    flex-direction: column;
  }

  .flex-rest {
    flex-grow: 99;
  }

  .log-textarea {
    overflow: auto;
    padding: 4px;
    background-color: #2e2e2e;
    color: white;
    border: solid grey 3px;
    border-radius: 4px;
  }

</style>
