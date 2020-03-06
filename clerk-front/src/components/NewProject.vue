<template>
  <v-card>
    <v-toolbar color="success" dark style="border-radius: inherit">
      <v-toolbar-title>
        New Project
      </v-toolbar-title>
      <v-spacer/>
      <v-btn icon @click="expand = !expand">
        <v-icon>{{expand ? 'mdi-chevron-up' : 'mdi-plus-box'}}</v-icon>
      </v-btn>
    </v-toolbar>
    <v-expand-transition>
      <div v-show="expand">
        <v-card-text>
          <v-form>
            <v-text-field label="Name" v-model="name"></v-text-field>
            <v-textarea outlined auto-grow label="Comment" v-model="comment"></v-textarea>
          </v-form>
        </v-card-text>
        <v-card-actions class="justify-center">
          <v-btn color="primary" :loading="working" @click="create">Create</v-btn>
        </v-card-actions>
      </div>
    </v-expand-transition>
  </v-card>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from 'vue-property-decorator'
    import {addNewProject} from "@/io/Project";

    @Component
    export default class NewProject extends Vue {
        name = '';
        comment = '';
        working = false;
        expand = false;
        async create() {
          this.working = true;
          const project = await addNewProject({
              name: this.name,
              comment: this.comment
          });
          this.$emit('create', project);
          this.working = false;
        }
    }
</script>

<style scoped>

</style>
