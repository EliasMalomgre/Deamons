package be.kdg.stemtest.model.entity

import androidx.room.Entity


@Entity(primaryKeys = arrayOf("id","statementId"))

data class AnswerOption(val id:Int,
                        var statementId:Int = -1,
                        val opinion:String,
                        val correct:Boolean=false)