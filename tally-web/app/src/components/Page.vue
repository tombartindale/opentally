<template lang="pug">
v-ons-page
  v-ons-toolbar
    v-ons-back-button Back
    .center {{ $route.name }}
    .right
      slot(name="settings")
  slot
</template>

<script>
export default {
  name: "Page",
  components: {},
};
</script>
