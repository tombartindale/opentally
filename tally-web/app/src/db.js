import firebase from "firebase";
import "firebase/database";

export const db = firebase
  .initializeApp({ databaseURL: "https://obstally.firebaseio.com/" })
  .database();
