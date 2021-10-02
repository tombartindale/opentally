<template lang="pug">
Page
  template(v-slot:settings)
    v-ons-toolbar-button(icon="md-settings")
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
      },
    };
  },
  computed: {
    source() {
      return this.$route.params.source;
    },
    sourceName() {
      return this.instance.Sources[this.$route.params.source];
    },
    isRecording() {
      return true;
    },
    isPreview() {
      return true;
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
  background: red;
}
.preview {
  background: green;
}
.label-parent {
  position: absolute;
  bottom: 5px;
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
}
</style>
