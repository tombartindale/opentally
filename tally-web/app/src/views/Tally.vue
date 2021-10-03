<template lang="pug">
Page(:title="sourceName")
  template(v-slot:settings)
    v-ons-icon(
      icon="md-link",
      size="36px",
      :style="{ color: instance.Online ? 'green' : 'silver' }"
    )
  .indicator(:class="{ recording: isRecording, preview: isPreview }")
  .label-parent
    .label {{ sourceName }}
</template>

<script>
import { db } from "../db";
import Page from "../components/Page.vue";

export default {
  name: "Tally",
  data() {
    return {
      instance: {
        Sources: [],
        PreviewTally: [],
      },
    };
  },
  computed: {
    source() {
      return this.$route.params.source;
    },
    sourceName() {
      if (this.source == "po") return "Program Out";
      return this.instance.Sources[this.source];
    },
    isRecording() {
      return this.instance.Recording || this.instance.Streaming;
      // if (this.instance.Recording) {
      //   if (this.source == "po") return true;
      //   else if (this.instance.Tally.includes(this.sourceName)) return true;
      //   else return false;
      // } else return false;
    },
    isPreview() {
      if (
        this.source == "po" &&
        (this.instance.Recording || this.instance.Streaming)
      )
        return true;
      else if (this.instance.PreviewTally.includes(this.sourceName))
        return true;
      else return false;
    },
  },
  created() {
    this.$rtdbBind(
      "instance",
      db.ref("instances").child(this.$route.params.id)
    );
  },
  methods: {
    goTo(key) {
      this.$router.push({ path: `/${this.$route.params.id}/${key}` });
    },
  },
  components: { Page },
};
</script>

<style scoped>
.recording {
  /* outline-color: red; */
  animation: flash 1s infinite;
}

@keyframes flash {
  50% {
    outline-color: white;
  }
}

.preview {
  background: red !important;
  /* border-color: transparent; */
}
.label-parent {
  position: absolute;
  bottom: 3vh;
  width: 100%;
  text-align: center;
}
.label {
  display: inline-block;
  background: rgba(0, 0, 0, 0.6);
  color: white;
  border-radius: 3vh;
  padding: 10px;
  font-size: 3vh;
}
.indicator {
  height: 100%;
  width: 100%;
  background: #333;
  outline-style: solid;
  outline-width: 2vh;
  outline-offset: -2vh;
  outline-color: transparent;
}
</style>
