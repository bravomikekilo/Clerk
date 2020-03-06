<template>
  <v-content class="fill-height">
    <v-row class="fill-height mx-5">
      <v-col cols="6">
        <v-card>
          <v-card-title>Update Experiment Progress</v-card-title>
          <v-card-text>
            <v-form>
              <v-text-field label="experiment ID" v-model="experimentIdStr"/>
              <v-text-field label="total" v-model="totalStr"/>
              <v-text-field label="progress" v-model="progressStr"/>
            </v-form>
          </v-card-text>
          <v-card-actions class="justify-center">
            <v-btn :disabled="working" @click="update">Update</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
      <v-col cols="6" class="fill-height">

      </v-col>
    </v-row>
  </v-content>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from "vue-property-decorator";
    import {updateExperimentComment, updateExperimentProgress} from "@/io/Experiment";

    @Component
    export default class DebugView extends Vue {
        experimentIdStr = '';
        totalStr = '';
        progressStr = '';

        working = false;

        get experimentId() {
            return Number.parseInt(this.experimentIdStr)
        }

        get total() {return Number.parseInt(this.totalStr)}
        get progress() {return Number.parseInt(this.progressStr)}

        async update() {
            this.working = true;
            await updateExperimentProgress(this.experimentId, this.total, this.progress)
            this.working = false;
        }


    }
</script>

<style scoped>

</style>
