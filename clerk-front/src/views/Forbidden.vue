<template>
  <v-content class="fill-height">
    <v-row class="justify-center">
      <v-col cols="12" sm="8" md="8" class="align-center" style="display: flex; flex-direction: column">
        <h1 class="display-1 my-12" style="color: darkred;">
          Forbidden
        </h1>
        <v-btn color="primary" link to="/">To Index</v-btn>
      </v-col>
    </v-row>
  </v-content>
</template>

<script lang="ts">
    import {Vue, Component} from 'vue-property-decorator'

    @Component
    export default class ForbiddenView extends Vue {}
</script>

<style scoped>

</style>
