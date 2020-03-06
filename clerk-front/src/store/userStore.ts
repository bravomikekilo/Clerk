import {Module, VuexModule, Mutation, getModule} from "vuex-module-decorators";
import store from './store'


@Module({name: "user", dynamic: true, namespaced: true, store})
export default class User extends VuexModule {
    username: string | null = null;

    @Mutation
    public signIn(username: string) {
        this.username = username;
    }

    @Mutation
    signOut() {
        this.username = null;
    }
}

export const UserModule = getModule(User);
