package be.kdg.stemtest.model.entity

import androidx.room.Entity
import androidx.room.PrimaryKey


@Entity
class Party (
   @PrimaryKey val name: String,
   val orientation:String,
   val colour:String,
   val partyLeader:String,
   val logo:String,
   val partyMediaLink:String?,
   var chosen: Boolean = false
)