<template lang="pug">
.tally(:class="{ live: liveMe, preview: previewMe }") {{instance.Tally}}
    div(:class="{recording: liveOut}")
    span {{this.$root.$data}}
</template>

<script>
export default {
    name: 'Tally',
    props:['instance'],
    computed:{
        liveOut(){
            return this.instance.Streaming || this.instance.Recording;
        },
        liveMe(){
            if (this.instance && this.instance.Tally)
                return this.instance.Tally.includes(this.$root.$data.selectedTally) && (this.instance.Streaming || this.instance.Recording)
            else
                return false;
        },

        previewMe(){
            if (this.instance && this.instance.PreviewTally)
                return this.instance.PreviewTally.includes(this.$root.$data.selectedTally)
            else
                return false;
        }
    }
}
</script>

<style lang="stylus" scoped>
.tally {
    border: 10px transparent solid
}

.live {
    border: 5px red solid;
}

.recording {
	background: rgba(255, 82, 82, 1);
	border-radius: 50%;
	margin: 10px;
	height: 20px;
	width: 20px;

	box-shadow: 0 0 0 0 rgba(255, 82, 82, 1);
	transform: scale(1);
	animation: pulse 2s infinite;
}



@keyframes pulse {
	0% {
		transform: scale(0.95);
		box-shadow: 0 0 0 0 rgba(255, 82, 82, 0.7);
	}
	
	70% {
		transform: scale(1);
		box-shadow: 0 0 0 10px rgba(255, 82, 82, 0);
	}
	
	100% {
		transform: scale(0.95);
		box-shadow: 0 0 0 0 rgba(255, 82, 82, 0);
	}
}
</style>