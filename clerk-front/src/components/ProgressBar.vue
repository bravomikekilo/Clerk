<template>
  <div>
    <v-progress-linear height="20" rounded class="mb-5"
                       :value="value"
                       :indeterminate="indeterminate">
      <template v-if="indeterminate">
        Running
      </template>
      <template v-else>
        {{this.progress}} / {{this.total}}
      </template>
    </v-progress-linear>
    Start Time: {{startTimeStr}} <br>
    ETA: {{eta === null ? 'N/A' : formatTimeOffset(eta)}} <br>
    EndTime: {{expectedEndTime}}
  </div>
</template>

<script lang="ts">
    import {Vue, Prop, Component} from 'vue-property-decorator';

    @Component
    export default class ProgressBar extends Vue {
        @Prop() progress!: number;
        @Prop() total!: number | null;
        @Prop() startTime!: Date;
        @Prop({default: false}) finished = false;
        currentTime: number = Date.now();
        timerId: NodeJS.Timeout | null = null;

        get color(): string {
            if (this.finished) {
                if (this.total != this.progress) {
                    return 'amber';
                } else {
                    return 'success';
                }
            } else {
                return 'primary';
            }
        }

        get startTimeStr() {
            return this.startTime.toLocaleString();
        }

        get value(): number {
            if (this.total === null) {
                return 0;
            } else {
                return 100 * this.progress / this.total;
            }
        }

        get indeterminate(): boolean {
            return (this.total === 0 || this.total === null) && !this.finished
        }

        get startMilliseconds(): number {
            return this.startTime.getMilliseconds();
        }

        get eta(): number | null {
            if (this.total === null) {
                return null
            }
            const timePassed = (this.currentTime - this.startMilliseconds);
            return timePassed / this.progress * this.total;
        }

        get expectedEndTime(): string {
            const eta = this.eta;
            if (eta === null) {
                return 'N/A'
            } else {
                return new Date(this.currentTime + eta).toLocaleString();
            }
        }

        formatTimeOffset(milliseconds: number) {
            const totalSeconds = Math.floor(milliseconds / 1000);
            const totalMinutes = Math.floor(totalSeconds / 60);
            const totalHours = Math.floor(totalMinutes / 60);

            const days = Math.floor(totalHours / 60);
            const seconds = totalSeconds % 60;
            const minutes = totalMinutes % 60;
            const hours = totalHours % 24;

            const dayStr = days == 0 ? '' : `${days} ${days > 1 ? 'days ' : 'day '}`;
            const hourStr = hours == 0 ? '' : `${hours} ${hours > 1 ? 'hours ' : 'hour '}`;
            const minuteStr = minutes == 0 ? '' : `${minutes} ${minutes > 1 ? 'minutes ' : 'minute '}`;
            const secondStr = seconds == 0 ? '' : `${seconds} ${seconds > 1 ? 'seconds ' : 'second '}`;

            return `${dayStr}${hourStr}${minuteStr}${secondStr}`
        }

        created() {
            this.timerId = setInterval(() => this.currentTime = Date.now(), 1000);
        }



    }
</script>

<style scoped>

</style>
