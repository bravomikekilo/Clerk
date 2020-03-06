<template>
  <v-list>
    <template v-for="(project) in projects">
      <v-list-item :key="project.id" link :to="`/project/${project.id}`">
        <v-list-item-content>
          <v-list-item-title>{{project.name}}</v-list-item-title>
          <v-list-item-subtitle>
            Created at {{project.createTime.toLocaleDateString()}} {{project.createTime.toLocaleTimeString()}}
          </v-list-item-subtitle>
          <v-list-item-subtitle>
            {{project.comment == null ? '' : project.comment}}
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-divider :key="'div' + project.id"/>
    </template>
  </v-list>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from 'vue-property-decorator'
    import {Project} from "@/model/Project";
    import {getAllProject} from "@/io/Project";

    @Component
    export default class ProjectList extends Vue {
        @Prop({default: []}) projects!: Project[];
    }
</script>

<style scoped>

</style>
