<template>
  <v-card class="elevation-12">
    <v-toolbar color="primary elevation-3" dark flat>
      <v-toolbar-title>Sign Up</v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <v-form>
        <v-text-field v-model="username"
                      label="Username" name="username" prepend-icon="mdi-account"></v-text-field>
        <v-text-field v-model="password"
                      label="Password" name="password" prepend-icon="mdi-lock"></v-text-field>
        <v-text-field v-model="repeatPassword"
                      label="Repeat password" prepend-icon="mdi-lock"></v-text-field>
        <v-text-field v-model="email"
                      label="E-mail" name="email" prepend-icon="mdi-email"></v-text-field>
      </v-form>
      {{errorText}}
    </v-card-text>
    <v-card-actions class="justify-center">
      <v-btn color="primary" @click="signUp">Sign Up</v-btn>
      <v-btn link :to="{name: 'IndexSignIn'}">Sign In</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from 'vue-property-decorator'
    import {signUp} from '@/io/User'

    @Component
    export default class SignUp extends Vue {
        working = false;
        username = '';
        repeatPassword = '';
        password = '';
        email = '';
        errorText = '';

        async signUp() {
            this.working = true;
            try {
                await signUp(this.username, this.password, this.email);
                console.log()
            } catch (error) {
                console.log(error);
                console.log(error.message);
                const errorMsg = error.response.data.map((t: any) => t.description);
                this.errorText = 'sign Up failed! ' + errorMsg.join('\n')
            }
            this.working = false;
        }
    }
</script>

<style scoped>

</style>
