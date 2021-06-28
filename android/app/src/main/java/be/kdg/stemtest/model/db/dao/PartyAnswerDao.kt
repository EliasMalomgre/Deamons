package be.kdg.stemtest.model.db.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Transaction
import be.kdg.stemtest.model.entity.PartyAnswer
import io.reactivex.Single

@Dao
abstract class PartyAnswerDao{

    @Insert
   abstract fun insert(partyAnswer: PartyAnswer)

    @Query("SELECT * FROM partyanswer")
    abstract fun getAllPartyAnswers(): Single<List<PartyAnswer>>


    @Transaction
    open fun saveAllAnswers(answers:List<PartyAnswer>){
        for (answer in answers) {
            insert(answer)
        }
    }
}