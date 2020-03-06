<template>
  <v-app>
    <v-app-bar
      color="primary"
      app
      dark
    >
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title>Clerk</v-toolbar-title>

      <v-spacer/>
      <template v-if="$store.state.user.username !== null">
        <v-btn
          @click="logout"
          text
        >
          <span class="mr-2">{{$store.state.user.username}}</span>
          <v-icon>mdi-logout</v-icon>
        </v-btn>
      </template>
    </v-app-bar>
    <v-navigation-drawer v-model="drawer" absolute temporary>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title class="title">
            Clerk
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-divider></v-divider>
      <v-list dense nav>
        <v-list-item link to="/index">
          <v-list-item-icon>
            <v-icon>mdi-account</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Index</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item link to="/project">
          <v-list-item-icon>
            <v-icon>mdi-book-multiple</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Projects</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <router-view></router-view>
  </v-app>
</template>

<script lang="ts">
    import {Vue, Component} from 'vue-property-decorator'
    import {UserModule} from "@/store/userStore";
    import {refresh} from "@/io/User";

    @Component
    export default class App extends Vue {
        drawer = false;

        async created() {
          try {
              await refresh();
          } catch {

          }
        }

        logout() {
            console.log('try to log out')
        }
    }
</script>
