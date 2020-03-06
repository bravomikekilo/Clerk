<template>
  <v-content class="fill-height" fluid>
    <v-row justify="center" style="margin-right: 0; margin-left: 0;">
      <v-col cols="12" sm="8" md="4">
        <new-project @create="createProject"></new-project>
      </v-col>
    </v-row>
    <v-row justify="center" style="margin-left: 0; margin-right: 0;">
      <v-col cols="12" sm="8" md="4">
        <v-card outlined class="my-5">
          <v-card-title class="justify-space-between">
            Projects
            <v-progress-circular v-if="loading" indeterminate color="primary">
            </v-progress-circular>
          </v-card-title>
          <v-card-text>
            <project-list :projects="projects"></project-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-content>
</template>

<script lang="ts">
    import {Vue, Component} from 'vue-property-decorator'
    import NewProject from "@/components/NewProject.vue";
    import ProjectList from "@/components/ProjectList.vue";
    import {Project} from "@/model/Project";
    import {getAllProject} from "@/io/Project";

    @Component({
        components: {
            NewProject,
            ProjectList
        }
    })
    export default class ProjectIndexView extends Vue {
        projects: Project[] = [];
        loading = false;

        async updateProjectList() {
            this.loading = true;
            this.projects = await getAllProject();
            this.loading = false;
        }

        async createProject(n: Project) {
           await this.updateProjectList();
        }

        async created() {
            await this.updateProjectList();
        }
    }
</script>

<style scoped>

</style>
