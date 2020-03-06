<template>
  <div style="display: flex; flex-direction: column;">
    <v-form v-if="editing">
      <v-textarea outlined auto-grow label="new comment" v-model="newComment"/>
      <v-btn color="primary" :loading="working" @click="saveComment" class="mr-4">Save</v-btn>
      <v-btn color="success" class="mr-4" @click="reset">Reset</v-btn>
      <v-btn class="mr-4" @click="cancel">Cancel</v-btn>
    </v-form>
    <template v-else>
      <slot>
        <p style="flex-grow: 1;" class="text--primary body-1">
          {{comment}}
        </p>
      </slot>
      <v-btn @click="editing = !editing" style="align-self: center">{{this.comment ? 'edit' : 'add comment'}}</v-btn>
    </template>
  </div>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from 'vue-property-decorator'

    @Component
    export default class CommentEditor extends Vue {
        @Prop() comment!: string;
        editing = false;
        working = false;
        newComment = '';

        created() {
            this.newComment = this.comment;
        }

        saveComment() {
            this.working = true;
            this.$emit('edit', this.newComment);
            this.working = false;
            this.editing = false;
        }

        reset() {
            this.newComment = this.comment;
        }

        cancel() {
            this.editing = false;
            this.newComment = this.comment;
        }

    }
</script>

<style scoped>

</style>
