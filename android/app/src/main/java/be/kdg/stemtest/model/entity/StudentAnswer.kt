package be.kdg.stemtest.model.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity
data class StudentAnswer(@PrimaryKey val id:Int,
                         var argument:String?,
                         val chosenAnswerId:Int,
                         var studentSessionId:Int?
                )