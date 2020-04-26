<template>
  <div id="app">
    <!-- <router-view></router-view> -->
    <v-ons-navigator swipeable
    :page-stack="pageStack"
    @push="pageStack = [...pageStack, $event]"
    @pop="pageStack.pop()"
    
  ></v-ons-navigator>
  </div>
</template>

<script>
import Instances from './components/Instances'

export default {
  name: 'App',
  data() {
    return {
      pageStack: [Instances],
    }
  },
  components: {
    
  }
}
</script>

<style lang="scss">
#app {

}
</style>
