<template>
  <v-card>
    <v-card-title>
      #{{experiment.id}} {{experiment.name}}
      <v-spacer/>
      <v-btn class="mx-3" link :to="`/experiment/${experiment.id}`">Detail</v-btn>
      <v-btn icon @click="expand = !expand">
        <v-icon>{{expand ? 'mdi-chevron-up' : 'mdi-chevron-down'}}</v-icon>
      </v-btn>
    </v-card-title>
    <v-card-text>
      <div>
        <v-progress-linear height="20" rounded class="mb-5"
                           :value="value"
                           :indeterminate="indeterminate">
          <template v-if="!indeterminate">
            {{this.experiment.progress}} / {{this.experiment.total}}
          </template>
          <template v-else>
            Running
          </template>
        </v-progress-linear>
        ETA: 3min
      </div>
    </v-card-text>
    <v-expand-transition>
      <div v-show="expand" style="display: flex; flex-direction: column;">
        <v-divider></v-divider>
        <v-card-text>
          <strong>Config name:</strong> {{this.experiment.configName}} <br>
          <v-divider></v-divider>
          <strong>Driver:</strong> {{this.experiment.driver}}
          <v-divider></v-divider>
          <strong>Command:</strong> {{this.experiment.command}}
          <v-divider></v-divider>
          <strong>Start time:</strong> {{this.experiment.startTime.toLocaleString()}}
          <v-divider></v-divider>
          <strong>Git hash:</strong> {{this.experiment.gitHash ? this.experiment.gitHash : "no git hash"}}
        </v-card-text>
        <v-card-actions class="justify-center py-3" style="display: flex">
          <v-dialog
            v-model="dialog"
            width="500"
          >
            <template v-slot:activator="{ on }">
              <v-btn color="error" dark v-on="on">Finish</v-btn>
            </template>
            <v-card>
              <v-card-title
                class="headline grey lighten-2"
              >
                Confirm finish
              </v-card-title>
              <v-card-text>
                A you sure to finish tracking experiment? <br>
                Process on server <strong>will not</strong> be terminate!
              </v-card-text>
              <v-divider/>
              <v-card-actions>
                <v-spacer/>
                <v-btn color="error" dark @click="finishExperiment">Finish</v-btn>
                <v-btn @click="dialog = false">Cancel</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>

        </v-card-actions>
      </div>
    </v-expand-transition>

  </v-card>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from 'vue-property-decorator'
    import {Experiment} from "@/model/Experiment";

    @Component
    export default class RunningExperiment extends Vue {
        @Prop() experiment!: Experiment;
        expand = false;
        dialog = false;

        get value(): number {
            if (this.experiment.total === null) {
                return 0;
            } else {
                return 100 * this.experiment.progress / this.experiment.total
            }
        }

        get indeterminate(): boolean {
            return (this.experiment.total == 0 || this.experiment.total === null);
        }

        finishExperiment() {
          this.dialog = false;
          console.log(`finish experiment #${this.experiment.id}`)
        }
    }
</script>

<style scoped>

</style>
