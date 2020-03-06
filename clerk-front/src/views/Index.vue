<template>
  <v-container class="fill-height" fluid>
    <v-row align="center" justify="center" style="margin-left: 0; margin-right: 0;">
      <v-col cols="12" sm="8" md="4">
        <router-view></router-view>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
  import {Vue, Prop, Component } from "vue-property-decorator";

  @Component
  export default class IndexView extends Vue {

  }
</script>

<style scoped>

</style>
