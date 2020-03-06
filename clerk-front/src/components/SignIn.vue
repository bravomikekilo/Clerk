<template>
  <v-card class="elevation-12">
    <v-toolbar color="primary elevation-3" dark flat>
      <v-toolbar-title>Sign In</v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <v-form>
        <v-text-field v-model="username"
          label="Username"
          name="username" prepend-icon="mdi-account"/>
        <v-text-field v-model="password"
          label="Password"
          name="password" prepend-icon="mdi-lock" type="password"/>
      </v-form>
      {{ errorText }}
    </v-card-text>
    <v-card-actions class="justify-center">
      <v-btn color="primary" @click="signIn" :loading="working">Sign In</v-btn>
      <v-btn link :to="{name: 'IndexSignUp'}">Sign Up</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from "vue-property-decorator";
    import {signIn, signUp} from "@/io/User";
    import {Getter, Mutation, namespace} from 'vuex-class';
    import { UserModule } from '@/store/userStore'

    const User = namespace('user');

    @Component
    export default class SignIn extends Vue {
        working = false;
        username = '';
        password = '';
        errorText = '';

        async signIn() {
            this.working = true;
            try {
                await signIn(this.username, this.password);
                UserModule.signIn(this.username);
                console.log('login success')
            } catch (error) {
                this.errorText = "login failed"
            }
            this.working = false;
        }
    }
</script>

<style scoped>

</style>
