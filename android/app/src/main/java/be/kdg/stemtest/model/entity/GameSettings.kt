package be.kdg.stemtest.model.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity
data class GameSettings( @PrimaryKey(autoGenerate = true)  val id:Int,
                         val amountOfStatements:Int,
                         val argumentsAllowed:Boolean,
                         val definitionsGiven:Boolean,
                         val skipAllowed:Boolean,
                         val gameType:Int,
                         val colour1: String?,
                         val colour2:String?,
                         val colour3:String?,
                         val colour4:String?,
                         val colour5: String?,
                         val colour6: String?,
                         val colourSkip: String?
) {
}